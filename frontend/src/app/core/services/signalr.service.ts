import { Injectable } from '@angular/core';
import { UserConnection, Hub } from '../models';
import { AppStateService } from './app-state.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private userConnections: UserConnection[] = [];

  constructor(private appState: AppStateService) {}

  connect(groupName: string, hub: Hub) {
      debugger;
      let targetConnection = this.userConnections.find(c => c.hub == hub);
      if (!targetConnection) {
          // создаем подключение
          targetConnection = new UserConnection(hub, this.appState);
          this.userConnections.push(targetConnection);
          targetConnection.isClosedByUser.subscribe(hub => {
              this.userConnections = this.userConnections.filter(
                  c => c.hub != hub
              );
          });
          targetConnection.connect().then(d => {
              targetConnection.joinGroup(groupName);
          });
      } else {
          targetConnection.joinGroup(groupName);
      }

      return targetConnection;
  }

  leaveGroup(groupName: string, hub: Hub) {
      const targetConnection = this.userConnections.find(c => c.hub === hub);
      if (targetConnection) {
          targetConnection.leaveGroup(groupName);
      }
  }
}
