import { Injectable } from '@angular/core';
import { SnotifyService, SnotifyToast } from 'ng-snotify';
import { AppStateService } from './app-state.service';
import { Invocation, UserConnection } from '../models/signalR';
import { Observable } from 'rxjs';
import { ChessGameService } from './chess-game.service';
import { GameSettings, GameOptions, OpponentType, StyleOptions } from '../models';
import { ActivatedRoute, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  /// Initialize this filds with data from the database!!
  private invocationsRecieved: Invocation[] = [];
  private invocationsSent: Invocation[] = [];
  private unreadNotificationsCount: number = 0;

  constructor(
    private snotifyService: SnotifyService,
    private appStateService: AppStateService,
    private chessGameService: ChessGameService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.appStateService
      .getSignalRConnectionObs()
      .subscribe(
        (connection) => {
          if (connection) {
            this.subscribeNotifications(connection);
          }
        })
  }

  subscribeNotifications(signalRConnection: UserConnection) {
    signalRConnection.onInvocationReceived(
      (invocation: Invocation) => {
        this.notifyInvocationReceived(invocation);
      });
  }

  notifyInvocationReceived(invocation: Invocation) {
    debugger;
    this.invocationsRecieved.push(invocation);
    if (!this.appStateService.isNotificationsMuted) {

      const yesAction = (toast: SnotifyToast) => {

        // присоедениться к игре на сервере
        // обновить игру в appStateService
        // перейти на game page

        this.chessGameService.joinGame(invocation.gameId).subscribe(game => {
          if (game) {
            const selectedSide = game.sides.find(s => s.player.uid === this.appStateService.getCurrentUser().uid);
            const gameOptions = new GameOptions(
              true,
              selectedSide.color,
              OpponentType.Player,
              game.sides.find(s => s.player.uid !== selectedSide.player.uid).player
            );
            let currentGame = new GameSettings(new StyleOptions(), gameOptions, game.fen);
            currentGame.gameId = game.id;
            this.appStateService.currentGame = currentGame;

              if(this.router.url !== '/play')
              {
                this.router.navigate(['/play']);
              }
          } else {
            throw new Error("User has not joined to game.ERROR")
          }
        });

        this.snotifyService.remove(toast.id);
      }

      const noAction = (toast: SnotifyToast) => {
        this.appStateService.signalRConnection.dismissInvocation(invocation.inviter.uid);
        this.snotifyService.remove(toast.id) // default
      }

      this.snotifyService.confirm(`${invocation.inviter.name} invited you to a game of chess!`, 'Invitation', {
        timeout: 120000,
        showProgressBar: true,
        closeOnClick: false,
        pauseOnHover: true,
        buttons: [
          { text: 'Accept', action: yesAction, bold: true },
          { text: 'Dismiss', action: noAction },
        ]
      });


    }
    // добавить invocation в список
    // если !isNotificationsMuted инициализировать snotify
    // с помощью snotify послать сигнал на сервер


    // получить инфу от snotify и оповестить appStateService о необходимости изменения currentGame
    // оповестить Navigation о необходимости перейти на gameComponent
    // gameComponent инициализирует игру 


  }

  notifyInvocationDismissed() {
    // удалить данное приглашение
    // сообщить отправителю 
  }

  notifyInvocationAccepted() {

  }

  notifyInvocationCanceled() {
    // удалить snotify
    // добавить в notifcationWindow информацию о том что данное приглашение отменено
  }
}
