import { Injectable } from '@angular/core';
import { SnotifyService } from 'ng-snotify';
import { AppStateService } from './app-state.service';
import { Invocation } from '../models/signalR';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {

  constructor(
    private appStateService: AppStateService,
    private snotifyService: SnotifyService
    ) 
    {
      this.appStateService.signalRConnection.onInvocationReceived(
        (invocation: Invocation) => {
        this.notifyInvocationReceived(invocation);
      });

    }

    notifyInvocationReceived(invocation: Invocation) {
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
