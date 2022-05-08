import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../services/user.service';

//import { Token } from '../token.model';
import { map } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Token } from '../models/token.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  public loginForm=this.formBuilder.group({
    userName: ['', Validators.required], 
    password: ['', Validators.required]
  })


  constructor(private formBuilder: FormBuilder,
  private userService:UserService, private toastr:ToastrService, private router: Router) { }

  token: Token = new Token();//
  
  ngOnInit(): void {
  }
  onSubmit(){
    console.log("on Submit");
    let userName = this.loginForm.value.userName;
    let password = this.loginForm.controls["password"].value;

     this.userService.login(userName, password)
    .pipe(map((data) => data))
    .toPromise()
    .then(res => {
      //this.token1 = res as Token;
      let token2 = res as Token;
      localStorage.setItem("access-token", token2.token);
      let role = token2.roles;;
      if(role === "Admin")
      {
          this.router.navigate(["/user-management"]);
      }
      else if(role === "OrdinaryUser")
      {
        this.router.navigate(["/user-management"]);
      }
      //this.toastr.success("Record Deleted Successfully", "Response Status!"); 
      // redirect to appropriate secured page
    }).catch((error: HttpErrorResponse) => {
      localStorage.setItem("access-token", "token2.token");
          this.toastr.error(error.error.Message, "Response Status!")           
      });
  }
}
