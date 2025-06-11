import { HttpClient } from "@angular/common/http";
import { inject } from "@angular/core";
import { Observable } from "rxjs";

export class ProductService {
    private http = inject(HttpClient);

    getProduct(id:number=1){
        return this.http.get('https://dummyjson.com/products/'+id)
    }

    getAllProducts():Observable<any[]>{
        return this.http.get<any[]>('https://dummyjson.com/products');
    }

}