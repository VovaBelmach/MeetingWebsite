import { Injectable } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class FriendService {
    constructor(private fb: FormBuilder, private http: HttpClient) { }
    readonly BaseURI = 'https://localhost:44333/api';

    getNewRequest(){
        return this.http.get(this.BaseURI + '/friend/ListNewRequests');
    }

    AcceptNewRequest(id: number){
        return this.http.get(this.BaseURI + '/friend/AcceptNewRequest/' + id.toString());
    }

    RejectNewRequest(id: number){
        return this.http.get(this.BaseURI + '/friend/RejectNewRequest/' + id.toString());
    }

    getFriendList(){
        return this.http.get(this.BaseURI + '/friend/Friends');
    }

    DeleteFriend(friendshipId: number){
        return this.http.get(this.BaseURI + '/friend/DeleteFriend/' + friendshipId.toString());
    }

    getSearchUserDetails(userId: string){
        return this.http.get(this.BaseURI + '/friend/FriendInfo/' + userId)
    }

    checkFriend(userId: string){
        return this.http.get(this.BaseURI + '/friend/CheckFriend/' + userId)
    }

    deleteFriendFromProfile(userId: string){
        return this.http.get(this.BaseURI + '/friend/DeleteFriendFromProfile/' + userId);
    }
}