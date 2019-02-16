import { Pipe, PipeTransform } from "@angular/core";
import { DatePipe } from "@angular/common";

@Pipe({
	name: "lastEntryDate"
})
export class LastEntryDatePipe extends DatePipe implements PipeTransform {
	transform(
		value: any,
		format?: string,
		timezone?: string,
		locale?: string
	): string | null {
		const date = new Date(value);
		const dateNow = new Date();

		if (date.getFullYear() === dateNow.getFullYear()) {
			if (date.getMonth() === dateNow.getMonth()) {
				const dateYesterday = new Date(dateNow.getTime() - 24 * 3600000);
				if (date.getDate() === dateNow.getDate()) {
          return ("Today at " + super.transform(value, "shortTime", timezone, locale));
				} else if (date.getDate() === dateYesterday.getDate()) {
					return ("Yesterday at " + super.transform(value, "shortTime", timezone, locale));
				}
			} else {
				return super.transform(value, "MMM d", timezone, locale);
			}
		} else {
      return super.transform(value, "'MMM d, y", timezone, locale);
    }
	}
}
