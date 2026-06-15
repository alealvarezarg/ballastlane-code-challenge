import { FormControl, FormGroup, Validators } from '@angular/forms';

export function createTaskQueryForm(): FormGroup<{
  search: FormControl<string>;
  status: FormControl<'Pending' | 'InProgress' | 'Completed' | ''>;
  userId: FormControl<string>;
  sortBy: FormControl<'title' | 'status' | 'dueDate' | ''>;
  sortDirection: FormControl<'asc' | 'desc' | ''>;
  pageSize: FormControl<number>;
  includeArchived: FormControl<boolean>;
}> {
  return new FormGroup({
    search: new FormControl('', { nonNullable: true }),
    status: new FormControl<'' | 'Pending' | 'InProgress' | 'Completed'>('', { nonNullable: true }),
    userId: new FormControl('', { nonNullable: true }),
    sortBy: new FormControl<'' | 'title' | 'status' | 'dueDate'>('', { nonNullable: true }),
    sortDirection: new FormControl<'' | 'asc' | 'desc'>('', { nonNullable: true }),
    pageSize: new FormControl(10, { nonNullable: true, validators: [Validators.min(1), Validators.max(200)] }),
    includeArchived: new FormControl(true, { nonNullable: true })
  });
}
