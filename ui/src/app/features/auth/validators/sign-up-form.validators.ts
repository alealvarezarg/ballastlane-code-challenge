import { FormControl, FormGroup, Validators } from '@angular/forms';

export function createSignUpForm(): FormGroup<{
  username: FormControl<string>;
  email: FormControl<string>;
  password: FormControl<string>;
}> {
  return new FormGroup({
    username: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required]
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email]
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(8)]
    })
  });
}
