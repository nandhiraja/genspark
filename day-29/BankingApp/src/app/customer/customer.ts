import { Component } from '@angular/core';
import { CustomerModel } from '../Models/customer.model';

@Component({
  selector: 'app-customer',
  imports: [],
  templateUrl: './customer.html',
  styleUrl: './customer.css',
})
export class Customer {

  customer : CustomerModel = new CustomerModel();
  PlaceHolder:string = "Enter value"
  CustomerName:string = "NandhiRaja"
  styclass: string = "bi bi-balloon-heart-fill";
  handleOnClick() {
    alert("Hello you clicked mee..!!")
  }

  toggleLike(){
    if(this.styclass === "bi bi-balloon-heart-fill"){
      this.styclass = "bi bi-balloon-heart";
    } else {
      this.styclass = "bi bi-balloon-heart-fill";
    }
  }
}
