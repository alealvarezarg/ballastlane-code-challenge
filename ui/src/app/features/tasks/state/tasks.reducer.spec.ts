import { tasksActions } from './tasks.actions';
import { tasksFeature } from './tasks.reducer';

describe('tasks reducer', () => {
  it('stores queried tasks', () => {
    const state = tasksFeature.reducer(undefined, tasksActions.loadSucceeded({
      items: [
        {
          id: '1',
          title: 'Task',
          description: 'Description',
          status: 'Pending',
          dueDate: '2099-01-01T00:00:00Z',
          userId: 'user-1',
          isArchived: false,
          archivedAt: null
        }
      ],
      totalCount: 1
    }));

    expect(state.tasks.length).toBe(1);
    expect(state.totalCount).toBe(1);
  });
});
