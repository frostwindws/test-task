import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

const signalRUrl = '/updates';

/**
 * Сервис соединения с SignalR для получения обновлений.
 */
@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: HubConnection;

  /**
   * Конструктор сервиса.
   */
  constructor() {
    let builder = new HubConnectionBuilder();

    this.hubConnection = builder.withUrl(signalRUrl).build();
    this.hubConnection.start();
  }

  /**
   * Подписка на получение сервисом сообщения.
   * @param type Тип сообщения.
   * @param callback Метод обработки сообщения.
   */
  public subscribe(type: string, scope, callback: (result) => void) {
    this.hubConnection.on(type, (result) => {
      callback.call(scope, result);
    });
  }
}
