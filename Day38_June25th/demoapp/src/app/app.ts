import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FileUploadComponent } from "./file-upload/file-upload";
import { ImageDemo } from "./image-demo/image-demo";


@Component({
  selector: 'app-root',
  imports: [FileUploadComponent, ImageDemo],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'demoapp';
}
