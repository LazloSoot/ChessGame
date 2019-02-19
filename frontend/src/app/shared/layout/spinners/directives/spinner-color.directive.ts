import { Directive, ElementRef, Renderer2, Input } from '@angular/core';

@Directive({
  selector: '[spinnerColor]'
})
export class SpinnerColorDirective {
@Input("spinnerColor") set color (value: string) {
  const targets = this.el.nativeElement.querySelectorAll('.spinner');
  targets.forEach(element => {
    this.renderer.setStyle(element, "background-color", value);
  });
}
  constructor(private el: ElementRef, private renderer: Renderer2) {
   }

}
