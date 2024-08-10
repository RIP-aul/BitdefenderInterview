import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { WebSocketService } from '../web-socket.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss'],
  standalone: true, // Declare as standalone component
  imports: [FormsModule] // Import FormsModule here if needed
})
export class ChatComponent {
  messages: string[] = [];
  messageInput: string = '';

  constructor(private webSocketService: WebSocketService) {
    this.webSocketService.messages$.subscribe((messages) => {
      this.messages = messages;
    });
  }

  sendMessage(): void {
    if (this.messageInput.trim()) {
      this.webSocketService.sendMessage(this.messageInput);
      this.messageInput = '';
    }
  }
}
