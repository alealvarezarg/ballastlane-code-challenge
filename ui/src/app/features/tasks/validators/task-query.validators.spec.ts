import { createTaskQueryForm } from './task-query.validators';

describe('createTaskQueryForm', () => {
  it('enforces page size upper bound', () => {
    const form = createTaskQueryForm();
    form.controls.pageSize.setValue(250);

    expect(form.controls.pageSize.valid).toBeFalse();
  });
});
