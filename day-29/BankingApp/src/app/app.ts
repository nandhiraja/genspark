import { Component, signal } from '@angular/core';
import { Customer } from './customer/customer';
import { Products } from './products/products';
import { Login } from './login/login';
import { Register } from './register/register';
import { Account } from './account/account';

@Component({
  selector: 'app-root',
  imports: [Customer,Products,Login , Register, Account],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('BankingApp');
}
