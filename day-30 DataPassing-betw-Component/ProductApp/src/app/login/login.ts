import { Component, signal } from '@angular/core';
import { BankingApiService } from '../services/banking.api.service';
import { LoginModel } from '../Models/login.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginModel = signal(new LoginModel());
  
  constructor(private bankingApiService: BankingApiService) {
  }

  handleLoginClick(){
    console.log("Login button clicked");
    this.bankingApiService.loginApiCall(this.loginModel()).subscribe({
      next: (response: any) => {
        console.log("Login successful", response);
        sessionStorage.setItem('token', response.token);
        alert("Login successful!")
      },
      error: (error) => {
        console.error("Login failed", error);
        alert("Login failed. Please try again.");
      }
    });
  }

}