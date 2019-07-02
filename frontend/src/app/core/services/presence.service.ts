import { Injectable } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/auth';
import * as firebase from 'firebase/app';
import { tap, map, throttleTime, flatMap } from 'rxjs/operators';
import { AngularFireDatabase } from '@angular/fire/database';
import { Observable, Subscription, fromEvent, timer } from 'rxjs'
import { PagedResult, User, Page } from '..';

@Injectable({
  providedIn: 'root'
})

export class PresenceService {
  private currentUser: firebase.User;
  private timer: Subscription;
  private subscriptions: Subscription[];
  constructor(
    private firebaseAuth: AngularFireAuth,
    private database: AngularFireDatabase
  ) {
    this.subscriptions = [this.timer];
    this.firebaseAuth.authState
      .pipe(
        tap(user => {
          if (user) {
            this.currentUser = user;
            this.subscriptions[1] = this.updateOnConnect();
            this.subscriptions[2] = this.updateOnIdle();
            this.updateOnDisconnect();
          } else {
            this.currentUser = null;
            this.subscriptions.forEach((sub) => {
              if (sub)
                sub.unsubscribe();
            });
          }
        })
      )
      .subscribe();
  }

  private initUserData() {
    if (!this.currentUser) return;
    firebase.database().ref('users/' + this.currentUser.uid).set({
      name: this.currentUser.displayName,
      email: this.currentUser.email,
      status: "online",
      lastSeen: firebase.database.ServerValue.TIMESTAMP
    });
  }

  private updateOnConnect() {
    return this.database.object('.info/connected').valueChanges()
      .pipe(tap(connected => {
        if (connected) {
          
      this.getOnlineUsers(new Page(0,2));////////////////////////////////////////////////////////////////////////////////
          this.initUserData();
        }
      }))
      .subscribe();
  }

  private updateOnDisconnect() {
    firebase.database().ref().child(`users/${this.currentUser.uid}`)
      .onDisconnect()
      .update({ status: 'offline', lastSeen: firebase.database.ServerValue.TIMESTAMP });
  }

  private updateOnIdle() {
    return fromEvent(document, 'mousemove')
      .pipe(
        throttleTime(2000),
        tap(() => {
          if (this.currentUser)
            firebase.database().ref('users/' + this.currentUser.uid).update({ status: 'online' });
          this.resetTimer();
        })
      ).subscribe();
  }

  private resetTimer() {
    if (this.timer) this.timer.unsubscribe();
    this.timer = timer(5000).pipe(
      tap(() => {
        if (this.currentUser)
          firebase.database().ref('users/' + this.currentUser.uid).update({ status: 'away', lastSeen: firebase.database.ServerValue.TIMESTAMP });
      })).subscribe()
  }

  public getOnlineUsers(page?: Page): PagedResult<User> {
    debugger;
    let a = firebase.database().ref('users/')
    .orderByChild("status")
    //.startAt(page.pageIndex)
    //.endAt(page.pageIndex + page.pageSize)
    .once('value', function(snapshot) {
      debugger;
      snapshot.forEach(function(childSnapshot) { 
        var childKey = childSnapshot.key;
    var childData = childSnapshot.val();
    console.log(childData);
      })
    }).then((data) => {
      debugger; 
    });
    let b = firebase.database().ref('users/' + this.currentUser.uid);
    return null;
  }
}
