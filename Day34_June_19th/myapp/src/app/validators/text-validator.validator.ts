import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";


export function textValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

   
    if (value && value.length < 6) {
      return { lenError: 'Password is of wrong length' };
    }

    return null;
  };
}
