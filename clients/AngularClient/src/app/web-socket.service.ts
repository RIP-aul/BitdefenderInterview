import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private socket: WebSocket;
  private messagesSubject = new BehaviorSubject<string[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  constructor() {
    this.socket = new WebSocket('ws://localhost:5000/ws');

    this.socket.onopen = () => {
      console.log('WebSocket connection established.');
      // Optionally send an initial message
      this.sendMessage('Hello from Angular client!');
    };

    this.socket.onmessage = (event) => {
      console.log('Message from server:', event.data);
      const currentMessages = this.messagesSubject.value;
      this.messagesSubject.next([...currentMessages, event.data]);
    };

    this.socket.onclose = () => {
      console.log('WebSocket connection closed.');
    };

    this.socket.onerror = (error) => {
      console.error('WebSocket error:', error);
    };
  }

  sendMessage(message: string): void {
    this.socket.send(message);
  }
}