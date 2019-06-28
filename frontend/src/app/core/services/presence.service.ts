import { Injectable } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/auth';
import * as firebase from 'firebase/app';
import { AppStateService } from './app-state.service';
import { tap, map } from 'rxjs/operators';
import { AngularFireDatabase } from '@angular/fire/database';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  private currentUser: firebase.User;
  constructor(
    private firebaseAuth: AngularFireAuth,
    private database: AngularFireDatabase
  ) {
    this.firebaseAuth.authState
      .pipe(
        tap(user => {
          if (user) {
            this.currentUser = user;
            this.updateOnConnect();
            this.updateOnDisconnect();
          }

        })
      )
      .subscribe();
  }

  private initUserData() {
    if (!this.currentUser) return
    firebase.database().ref('users/' + this.currentUser.uid).set({
      name: this.currentUser.displayName,
      email: this.currentUser.email,
      status: "online"
    });
  }

  private updateOnConnect() {
    return this.database.object('.info/connected').valueChanges()
      .pipe(map(connected => {
        if (connected) {
          this.initUserData();
        }
      }))
      .subscribe();
  }

  private updateOnDisconnect() {
    firebase.database().ref().child(`users/${this.currentUser.uid}`)
      .onDisconnect()
      .update({ status: 'offline' })
  }
}
