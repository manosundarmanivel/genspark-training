import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BulkInsertService } from '../bulk-insert.service'
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.html',
  imports:[JsonPipe],
  providers:[BulkInsertService]
})
export class FileUploadComponent {
  constructor(private http: HttpClient) {}
 private service =  inject(BulkInsertService);
  insertedRecords:any;

  handleFileUpload(event: any) {
  const file = event.target.files[0];
  this.service.processData(file).subscribe({
    next:(data)=>this.insertedRecords= data,
    error:(err)=>alert(err)

  })
  

  }
  

}