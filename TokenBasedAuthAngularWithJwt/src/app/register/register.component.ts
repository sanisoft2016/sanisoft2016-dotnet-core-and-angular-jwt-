import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../services/user.service';
import { map } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { HttpCustomResponseModel } from '../models/HttpCustomResponseModel';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  public registerForm=this.formBuilder.group({
    userCategory: ['', [Validators.required]],
    userName:  ['', Validators.required],
    firstName: ['', Validators.required],
    surName: ['', Validators.required],
    phoneNumber: ['', Validators.required], 
    email: ['', [Validators.email, Validators.required]], 
    
    password: ['', Validators.required],
    confirmPassword: ['', Validators.required] 
  })

  constructor(private formBuilder: FormBuilder, 
    private userService:UserService, private toastr:ToastrService) { }
  
  ngOnInit(): void {
  }
  onSubmit(){
    console.log("on Submit");
    let userCategory = Number(this.registerForm.value.userCategory);
    let userName = this.registerForm.value.userName;
    let firstName = this.registerForm.value.firstName;
    let surName = this.registerForm.controls["surName"].value
    let phoneNumber = this.registerForm.controls["phoneNumber"].value

    let email = this.registerForm.controls["email"].value
    let password = this.registerForm.controls["password"].value
    let confirmPassword = this.registerForm.value.confirmPassword;
    this.userService.register(userCategory, userName, firstName, surName,  phoneNumber, email, 
      password, confirmPassword)
      .pipe(map((data) => data))
      .toPromise()
      .then((response) => {
        let result = response as HttpCustomResponseModel
            this.toastr.success(result.message, "Response Status!"); 
          })
          .catch((error: HttpErrorResponse) => {
            this.toastr.error(error.error.message, "Response Status!")           
        });
  }
}
