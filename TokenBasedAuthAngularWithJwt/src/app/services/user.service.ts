import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly baseURL ="https://localhost:5001/api/Authentication/";

  constructor(private httpClient: HttpClient) { }

  public login (userName:string, password:string){
    const body={
      UserName:userName,
      Password:password
    }
    return this.httpClient.post(this.baseURL + "Login", body)
  }

  public register (userCategory: number, userName: string, firstName:string,surName:string,phoneNumber:string, email:string, password:string, confirmPassword:string){
    const register={
      UserCategory: userCategory,
      UserName: userName,
      FirstName:firstName,
      SurName:surName,
      PhoneNumber:phoneNumber,
      Email:email,
      ConfirmPassword:confirmPassword,
      Password:password
    }
    return this.httpClient.post(this.baseURL + "Register", register);
  }

  public getAllUsers(){
    const header = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem("access-token")}`
    })
    let token =  localStorage.getItem("access-token");
    return this.httpClient.get(this.baseURL + "GetUsers", {headers:header});
  }

  public deleteUser(userId:string){
    const header = new HttpHeaders({
      'Authorization': `Bearer ${localStorage.getItem("access-token")}`
    })
    let url = `${this.baseURL}DeleteUser?userName=${userId}`;
    return this.httpClient.delete( url, {headers:header});
  }
}
