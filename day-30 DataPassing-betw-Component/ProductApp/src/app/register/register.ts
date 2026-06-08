import { Component, signal } from '@angular/core';
import { RegisterModel } from '../Models/register.model';
import { BankingApiService } from '../services/banking.api.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  registerModel = signal(new RegisterModel());
  
 constructor(private bankingApiService: BankingApiService) {
  }   
   
   handleRegisterClick(){
    this.bankingApiService.registerApiCall(this.registerModel()).subscribe({
      next: (response) => {
        console.log("Registration successful", response);
        alert("Registration successful!")
      },
      error: (error) => {
        console.error("Registration failed", error);
        alert("Registration failed. Please try again.");
      }
    });
  } 
}
