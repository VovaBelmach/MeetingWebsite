import { Injectable } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private fb: FormBuilder, private http: HttpClient) { }

  formModel = this.fb.group({
    FirstName: ['', Validators.required],
    LastName: [''],
    Email: ['', Validators.email],
    Passwords: this.fb.group({
      Password: ['', [Validators.required, Validators.minLength(4)]],
      PasswordConfirm: ['', Validators.required],
    }, { validator: this.comparePasswords })
  })

  comparePasswords(fb: FormGroup) {
    let confirmPswrdCtrl = fb.get('PasswordConfirm');
    //passwordMismatch
    //confirmPswrdCtrl.errors={passwordMismatch: true}
    if (confirmPswrdCtrl.errors == null || 'passwordMismatch' in confirmPswrdCtrl.errors) {
      if (fb.get('Password').value != confirmPswrdCtrl.value)
        confirmPswrdCtrl.setErrors({ passwordMismatch: true });
      else
        confirmPswrdCtrl.setErrors(null);
    }
  }

  register() {
    var body = {
      FirstName: this.formModel.value.FirstName,
      LastName: this.formModel.value.LastName,
      Email: this.formModel.value.Email,
      Password: this.formModel.value.Password,
      PasswordConfirm: this.formModel.value.PasswordConfirm
    }
  }
}
