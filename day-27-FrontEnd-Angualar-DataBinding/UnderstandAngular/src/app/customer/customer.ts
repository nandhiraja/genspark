import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-customer',
  imports: [FormsModule],
  templateUrl: './customer.html',
  styleUrl: './customer.css',
})
export class Customer {
  protected name: string = 'User Name';

  // Method to update the name property 

  updateName(newName: string): void {
    this.name = newName;
  } 

  // signal 

  protected readonly nameSignal = signal('User Name');

}
