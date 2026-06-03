import { Component, signal } from '@angular/core';
import { ProductApiService } from '../services/product.api.service';
import { ProductModel } from '../Models/product.model';
import { debounceTime, distinctUntilChanged, Subject, switchMap } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-products',
  imports: [FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {
  products = signal<ProductModel[]>([]);
  searchTerm = new Subject<string>();
  searchTermValue: string = '';

  constructor(private productApiService: ProductApiService) {
    this.productApiService.getProductsFromDummyJson()
      .subscribe({
      next:(response: any) => {
        console.log(response.products);
        this.products.set(response.products);
      },
      error:(error) => {
        console.error(error);
      },
      complete:()=>{
        console.log("Request completed");
      }
    });


    this.searchTerm.pipe(
            debounceTime(300), 
            distinctUntilChanged(),
            switchMap((searchTermValue) => this.productApiService.searchProduct(searchTermValue))).subscribe({
              next:(response: any) => {
                console.log(response.products);
                this.products.set(response.products);
              },
              error:(error:any) => {
                console.error(error);
              },
              complete:()=>{
                console.log("Search Request completed");
              }
              });
            
  }

  searchValue(){
    this.searchTerm.next(this.searchTermValue);
  }
  
  handleChangeClick(){
    console.log("Change button clicked");
  }
}