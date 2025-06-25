import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImageDemo } from './image-demo';

describe('ImageDemo', () => {
  let component: ImageDemo;
  let fixture: ComponentFixture<ImageDemo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImageDemo]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImageDemo);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
