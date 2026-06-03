import { Component } from '@angular/core';
import { BankingApiService } from '../services/banking.api.service';

@Component({
  selector: 'app-account',
  imports: [],
  templateUrl: './account.html',
  styleUrl: './account.css',
})
export class Account {

  constructor(private bankApiService: BankingApiService) {}
  handleGetAccountDetails(acctNumber: string) {
    this.bankApiService.getAccountDetails(acctNumber).subscribe({
      next:(response)=>{
        console.log('Account details:', response);
      },
      error:(error)=>{
        console.error('Error fetching account details:', error);
      }
    });
  }

}
