import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class ProductApiService{
  constructor(private http: HttpClient) {}
  public getProductsFromDummyJson(){
    return this.http.get("https://dummyjson.com/products");
  }

  public searchProduct(searchWord: string){
    return this.http.get(`https://dummyjson.com/products/search?q=${searchWord}`);
  }
}