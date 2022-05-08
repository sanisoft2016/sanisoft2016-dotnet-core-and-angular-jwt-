import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { map } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ApplicationUser } from '../models/application-user';
import { HttpCustomResponseModel } from '../models/HttpCustomResponseModel';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  list!: ApplicationUser[];

  constructor(private userService:UserService, private toastr:ToastrService) { }

  ngOnInit(): void {
    this.userService.getAllUsers()
      .pipe(map((data) => data))
      .toPromise()
      .then(res =>
        {
            this.list = res as ApplicationUser[]
            let paulse = 1;
        })
      .catch((error: HttpErrorResponse) => {
        this.toastr.error(error.error.Message, "Response Status!")           
    });
  }

  deleteUserById(userName:string){
    if(confirm("Are you sure you want to detele this record?")){
      this.userService.deleteUser(userName)
      .pipe(map((data) => data))
      .toPromise()
      .then((response) => {
        let result = response as HttpCustomResponseModel
            this.toastr.error(result.message, result.status);
            let index = this.list.findIndex(x => x.userName === userName);
            this.list.splice(index, 1);
          })
          .catch((error: HttpErrorResponse) => {
            // Handle error
        });
    }
  }

}
