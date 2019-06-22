import { Component, OnInit } from '@angular/core';
import { SignalRService } from 'src/app/shared/signalR.service';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Input } from '@angular/core';
import { ChatService } from 'src/app/shared/char.service';
import { MessageInfo } from 'src/app/models/MessageInfo';
import { Message } from 'src/app/models/Message';

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

  message = '';
  messages: Message[] = new Array();
  messagesRealTime: Message[] = new Array();
 
  incomingMessage =  new Message();

  visibleDropZone = true;

  ngOnInit() {
    this.signalR.startConnection();
    this.addSendListener();
    this.addSendMyselfListener();

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
        debugger;
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
    debugger;
    var outgoingMessage  = new MessageInfo();
    outgoingMessage.DialogId = this.dialogId;
    outgoingMessage.ReceiverId = this.userId;
    outgoingMessage.Message = this.message;

    console.log(this.dialogId, this.userId, this.message);

    this.service.sendMessage(outgoingMessage).subscribe();
  }

  onOpenDropzone(){
    this.visibleDropZone = !this.visibleDropZone;
  }

  onFilesAdded(files: File[]) {
   
    files.forEach(file => {
      const reader = new FileReader();
   
      reader.onload = (e: ProgressEvent) => {
        const content = (e.target as FileReader).result;
      };
   
      reader.readAsText(file);

    });
  }
   
  onFilesRejected(files: File[]) {
    console.log(files);
  }
}
