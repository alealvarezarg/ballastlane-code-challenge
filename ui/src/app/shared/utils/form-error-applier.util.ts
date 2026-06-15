import { FormGroup } from '@angular/forms';

import { ApiErrorState } from '@app/shared/models/api-error.models';

export function applyApiErrors(form: FormGroup, error: ApiErrorState | null): void {
  if (!error) {
    return;
  }

  for (const validationError of error.errors) {
    const controlName = validationError.propertyName.charAt(0).toLowerCase() + validationError.propertyName.slice(1);
    const control = form.get(controlName);

    if (control) {
      control.setErrors({
        ...(control.errors ?? {}),
        api: validationError.message
      });
    }
  }
}
