import { FormControl, FormGroup, Validators } from '@angular/forms';

export function createTaskForm(): FormGroup<{
  id: FormControl<string>;
  title: FormControl<string>;
  description: FormControl<string>;
  status: FormControl<'Pending' | 'InProgress' | 'Completed'>;
  dueDate: FormControl<string>;
  userId: FormControl<string>;
}> {
  return new FormGroup({
    id: new FormControl('', { nonNullable: true }),
    title: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    description: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    status: new FormControl<'Pending' | 'InProgress' | 'Completed'>('Pending', { nonNullable: true, validators: [Validators.required] }),
    dueDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    userId: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });
}
