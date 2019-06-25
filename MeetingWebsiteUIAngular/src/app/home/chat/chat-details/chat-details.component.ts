import { Component, OnInit } from '@angular/core';
import { SignalRService } from 'src/app/shared/signalR.service';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Input } from '@angular/core';
import { ChatService } from 'src/app/shared/char.service';
import { MessageInfo } from 'src/app/models/MessageInfo';
import { Message } from 'src/app/models/Message';
import { EmojiModule, Emoji } from '@ctrl/ngx-emoji-mart/ngx-emoji'

@Component({
  selector: 'app-chat-details',
  templateUrl: './chat-details.component.html',
  styleUrls: ['./chat-details.component.css']
})
export class ChatDetailsComponent implements OnInit {

  @Input() userId: string;
  @Input() dialogId: number;

  constructor(private activateRoute: ActivatedRoute,
    public signalR: SignalRService,
    private router: Router,
    public service: ChatService) { }

  message :any;
  messages: Message[] = new Array();
  messagesRealTime: Message[] = new Array();
  messagePhotos: File[] = new Array();

  incomingMessage = new Message();

  visibleStikers = true;

  ngOnInit() {
    this.signalR.startConnection();
    this.addSendListener();
    this.addSendMyselfListener();
    this.addNewDialogListener();

    this.service.getDetailsUserDialogs(this.dialogId).subscribe(
      res => {
        this.messages = res as Message[];
      },
      err => {
        console.log(err);
      }
    )
  }

  addSendListener() {    
    this.signalR.hubConnection.on('Send', (data) => {
      this.incomingMessage = data as Message;
      this.messagesRealTime.push(this.incomingMessage);
    });
  }

  addSendMyselfListener() {
    this.signalR.hubConnection.on('SendMyself', (data) => {
      this.incomingMessage = data as Message;
      this.messagesRealTime.push(this.incomingMessage);
    });
  }

  addNewDialogListener() {
    this.signalR.hubConnection.on('AddNewDialog', (data) => {
      this.incomingMessage = data as Message;
      this.messagesRealTime.push(this.incomingMessage);
    });
  }

  onSendMessage() {
    var formData = new FormData();
    formData.append('DialogId', this.dialogId.toString());
    formData.append('ReceiverId', this.userId);
    formData.append('Message', this.message);
    this.messagePhotos.forEach(photo => {
      formData.append('Photo', photo);
    });
    
    this.service.sendMessage(formData).subscribe();
  }

  onOpenStikers() {
    this.visibleStikers = !this.visibleStikers;
  }

  onFilesAdded(files: File[]) {
    this.messagePhotos = files;
  }

  onFilesRejected(files: File[]) {
    console.log(files);
  }

  addEmoji(event){
    const { message } = this;
    const text = `${message}${event.emoji.native}`;
    this.message = text;
  }
}
