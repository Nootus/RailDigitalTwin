import { Injectable, Inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { TrainModel } from './models/train.model';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: HubConnection;
  private subject = new Subject<TrainModel[]>();

  constructor(@Inject('BASE_URL') private baseUrl: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(baseUrl + "trainhub")
      .build();

    this.hubConnection.on('timerEvent', (trains) => {
      this.subject.next(trains);
    });

    this.hubConnection.start();
  }
  getObservable() {
    return this.subject.asObservable();
  }
}
