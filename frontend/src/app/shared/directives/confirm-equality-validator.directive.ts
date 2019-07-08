import { Validator, NG_VALIDATORS, AbstractControl } from '@angular/forms';
import { Directive, Input } from '@angular/core';

@Directive({
  selector: '[appConfirmEqualityValidator]',
  providers: [{
    provide: NG_VALIDATORS,
    useExisting: ConfirmEqualityValidatorDirective,
    multi: true
}]
})
export class ConfirmEqualityValidatorDirective {
  @Input() appConfirmEqualityValidator: string;
  
  validate(control: AbstractControl): {[key: string]: any} | null {
      const controlToCompare = control.parent.get(this.appConfirmEqualityValidator);
      if (controlToCompare && controlToCompare.value !== control.value) {
          return {'NotEqual': true};
      }
      return null;
  }

}
