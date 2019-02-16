import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'lastEntryDate'
})
export class LastEntryDatePipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return null;
  }

}
