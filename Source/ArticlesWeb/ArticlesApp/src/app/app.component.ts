import { Component } from '@angular/core';
import { HubConnection } from '@aspnet/signalr';

/**
 * Корневой компонент приложения.
 */
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
  private hubConnection: HubConnection;

  /**
   * Заголовок страницы.
   */
  title = 'Artilces';

  /**
   * Инициализация компонента приложения.
   */
  ngOnInit() {
  }
}
