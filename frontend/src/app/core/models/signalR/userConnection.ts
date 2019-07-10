import { EventEmitter } from "@angular/core";
import { Hub } from './hub';
import { environment } from '../../../../environments/environment';
import * as signalR from '@aspnet/signalr';
import { ServerAction } from "./serverAction";
import { Group } from "./group";
import { ClientEvent } from "./clientEvent";
import { Invocation } from "./invocation";
import { GameSide } from "../chess";

export class UserConnection {
    private joinGroupAttemptsСount = maxJoinAttemptsСount;
    private connectAttemptsCount = maxJoinAttemptsСount;
    private subscribeAttemptsCount = maxJoinAttemptsСount;
    private groups: string[] = [];
    private isDisconnectedByUser: boolean;
     connection: any;
     isClosedByUser = new EventEmitter<Hub>(true);

    constructor(public hub: Hub, private token: string) {}

    joinGroup(groupName: string) {
        if (this.groups.includes(groupName)) {
            return;
        }

        // 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected'
        switch (this.connection.connection.connectionState) {
            case 1:
                console.log(
                    `Connection state is ok, joining to group ${groupName}`
                );
                this.connection.send("joinGroup", groupName);
                this.groups.push(groupName);
                this.joinGroupAttemptsСount = maxJoinAttemptsСount;
                break;
            case 0: {
                if (this.joinGroupAttemptsСount < 1) {
                    console.log(
                        `Maximum number of attempts to join group ${groupName} was exhausted`
                    );
                    this.joinGroupAttemptsСount = maxJoinAttemptsСount;
                } else {
                    console.log(`Connection is on connecting state`);
                    setTimeout(() => {
                        this.joinGroupAttemptsСount--;
                        this.joinGroup(groupName);
                        return;
                    }, 1000);
                }
                break;
            }
            case 2: {
                if (this.joinGroupAttemptsСount < 1) {
                    console.log(
                        `Maximum number of attempts to join group ${groupName} was exhausted`
                    );
                    this.joinGroupAttemptsСount = maxJoinAttemptsСount;
                } else {
                    console.log(`Connection is on reconnecting state`);
                    this.reconnect();
                    setTimeout(() => {
                        this.joinGroupAttemptsСount--;
                        this.joinGroup(groupName);
                        return;
                    }, 1000);
                }
                break;
            }
            case 4: {
                if (this.joinGroupAttemptsСount < 1) {
                    console.log(
                        `Maximum number of attempts to join group ${groupName} was exhausted`
                    );
                    this.joinGroupAttemptsСount = maxJoinAttemptsСount;
                } else {
                    console.log(`Connection is on disconnected state`);
                    this.joinGroupAttemptsСount--;
                    this.groups.push(groupName);
                    this.reconnect();
                }
                break;
            }
        }
    }

    dismissInvocation(targetUserUid: string): Promise<void> {
        return this.send(
            ServerAction.DismissInvocation,
            `${Group.User}${targetUserUid}`
        );
    }

    cancelInvocation(targetUserUid: string): Promise<void> {
        return this.send(
            ServerAction.CancelInvocation,
            `${Group.User}${targetUserUid}`
            );
    }

    leaveGroup(groupName: string) {
        groupName = groupName.trim();
        if (this.groups.includes(groupName)) {
            if (this.connection.connection.connectionState === 1) {
                console.log(`Disconnecting from group ${groupName}`);
                this.connection.send("leaveGroup", groupName);
            }
            this.groups = this.groups.filter(g => g !== groupName);
            if (this.groups.length < 1) {
                console.log(`Stoping SignalR connection to ${this.hub}`);
                this.isDisconnectedByUser = true;
                this.isClosedByUser.emit(this.hub);
                this.connection.stop();
            }
        }
    }


    onInvocationReceived(newMethod: (invocation: Invocation) => void) {
        this.on(
			ClientEvent.InvocationReceived,
			newMethod
		);
    }

    onInvocationAccepted(newMethod: (gameId: number) => void){
        this.on(
            ClientEvent.InvocationAccepted,
            newMethod
        );
    }

    onInvocationDismissed(newMethod: (byUserWithUid: string) => void) {
        this.on(
            ClientEvent.InvocationDismissed,
            newMethod
        );
    }

    onInvocationCanceled(newMethod: ()=> void) {
        this.on(
            ClientEvent.InvocationCanceled,
            newMethod
            );
    }

    onMoveComitted(newMethod: ()=> void){
        this.on(
            ClientEvent.MoveCommitted,
            newMethod
        );
    }

    onCheck(newMethod: (checkTo: GameSide)=> void) {
        this.on(
            ClientEvent.Check,
            newMethod
        );
    }

    onMate(newMethod: (mateTo: GameSide)=> void) {
        this.on(
            ClientEvent.Mate,
            newMethod
        );
    }

    off(methodName: string): void {
        if (this.connection) {
            this.connection.off(methodName);
            this.subscribeAttemptsCount = maxJoinAttemptsСount;
        } else if (this.subscribeAttemptsCount > 0) {
            setTimeout(() => {
                this.subscribeAttemptsCount--;
                this.off(methodName);
                return;
            }, 1000);
        }
    }

    offAll(): void {
        for(let m in this.connection.methods)
        {
            this.off(m);
        }
    }

    connect(): Promise<void> {
        if (this.connectAttemptsCount < 1) {
            console.log(
                `Maximum number of attempts to connect ${
                    this.hub
                } was exhausted`
            );
            return;
        } else {
            if (!this.connection) {
                console.log(`Creating SignalR ${this.hub} connection...`);

               this.connection = new signalR.HubConnectionBuilder()
                           .withUrl(`${environment.apiUrl}/${this.hub}`, {
                               accessTokenFactory: () =>
                                   this.token
                           })
                           .build();

                this.connection.onclose(err => {
                    console.log(`SignalR hub ${this.hub} disconnected.`);
                    setTimeout(() => {
                        if (!this.isDisconnectedByUser) {
                            this.connectAttemptsCount--;
                            this.reconnect();
                        }
                    }, 500);
                });
            }

            return this.reconnect();
        }
    }

    reconnect(): Promise<void> {
        if (!this.connection) {
            this.connect();
        } else {
            console.log(`SignalR hub ${this.hub} reconnection started...`);
            if (this.connection.connection.connectionState === 2) {
                const connPromise = this.connection.start();

                connPromise.catch(err => {
                    console.log("SignalR ERROR " + err);
                    setTimeout(() => {
                        this.connectAttemptsCount--;
                        this.reconnect();
                    }, 500);
                });

                connPromise.then(() => {
                    let groupsForJoin = this.groups;
                    this.groups = [];
                    for (let i = 0; i < groupsForJoin.length; i++) {
                        this.joinGroup(groupsForJoin[i]);
                    }
                    this.connectAttemptsCount = maxConnectAttemptsСount;
                });
                return connPromise;
            }
        }
    }
    
    private send(serverActionName: ServerAction | string, arg: string): Promise<void> {
        return this.connection.send(serverActionName, arg);
    }

    private on(methodName: string, newMethod: (...args: any[]) => void): void {
        if (this.connection) {
            this.connection.on(methodName, newMethod);
            this.subscribeAttemptsCount = maxJoinAttemptsСount;
        } else if (this.subscribeAttemptsCount > 0) {
            setTimeout(() => {
                this.subscribeAttemptsCount--;
                this.on(methodName, newMethod);
                return;
            }, 1000);
        }
    }
}

const maxJoinAttemptsСount = 25;
const maxConnectAttemptsСount = 50;