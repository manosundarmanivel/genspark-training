import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaymentFormComponent } from './payment-form';
import { ReactiveFormsModule } from '@angular/forms';
import { RazorpayService } from '../../core/razorpay';

describe('PaymentFormComponent', () => {
  let component: PaymentFormComponent;
  let fixture: ComponentFixture<PaymentFormComponent>;
  let razorpayServiceSpy: jasmine.SpyObj<RazorpayService>;

  beforeEach(() => {
    razorpayServiceSpy = jasmine.createSpyObj('RazorpayService', ['openCheckout']);

    TestBed.configureTestingModule({
      declarations: [PaymentFormComponent],
      imports: [ReactiveFormsModule],
      providers: [{ provide: RazorpayService, useValue: razorpayServiceSpy }]
    });

    fixture = TestBed.createComponent(PaymentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create form with 4 controls', () => {
    expect(component.paymentForm.contains('amount')).toBeTrue();
    expect(component.paymentForm.contains('name')).toBeTrue();
    expect(component.paymentForm.contains('email')).toBeTrue();
    expect(component.paymentForm.contains('contact')).toBeTrue();
  });

  it('should invalidate empty form', () => {
    expect(component.paymentForm.valid).toBeFalse();
  });

  it('should call razorpayService on valid form', () => {
    component.paymentForm.setValue({
      amount: 100,
      name: 'Test User',
      email: 'test@example.com',
      contact: '9876543210'
    });

    component.payNow();

    expect(razorpayServiceSpy.openCheckout).toHaveBeenCalled();
  });
});
