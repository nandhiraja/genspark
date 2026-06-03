import { HttpClient } from "@angular/common/http";
import { LoginModel } from "../Models/login.model";
import { baseUrl } from "../environment";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root' // <-- Add this line
})
export class BankingApiService {
    static loginApiCall(LoginModel:  LoginModel) {
      throw new Error('Method not implemented.');
    }
    constructor(private http: HttpClient) {
        console.log("BankingApiService instance created");
    }

    loginApiCall(loginModel: LoginModel) {
        let url = baseUrl+'/Authentication/Login';
        return this.http.post(url, loginModel);
    }
    registerApiCall(registerModel: any) {
        let url = baseUrl+'/Authentication/Register';
        return this.http.post(url, registerModel);
    }

    getAccountDetails(acctNumber: string) {
        let url = baseUrl+`/Account?accountNumber=${acctNumber}`;
        return this.http.get(url);
    }
}