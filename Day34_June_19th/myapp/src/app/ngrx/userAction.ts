
import { createAction, props } from '@ngrx/store';
import { User } from '../ngrx/userModel';

export const addUser = createAction(
  '[User] Add User',
  props<{ user: User }>()
);


