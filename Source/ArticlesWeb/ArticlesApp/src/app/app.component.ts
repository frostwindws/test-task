import { Component } from '@angular/core';
import { HubConnection } from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private hubConnection: HubConnection;
  title = 'ArticlesApp';
  ngOnInit() {

    /*this.hubConnection = new HubConnection('http://localhost:5000/chat');

    this.hubConnection
      .start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection :('));*/

  }
}
