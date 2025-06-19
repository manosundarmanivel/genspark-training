import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsMap } from './us-map';

describe('UsMap', () => {
  let component: UsMap;
  let fixture: ComponentFixture<UsMap>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsMap]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsMap);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
