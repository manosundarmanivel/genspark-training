
import { createReducer, on } from '@ngrx/store';
import { addUser } from '../ngrx/userAction';
import { User } from '../ngrx/userModel';

export interface UserState {
  users: User[];
}

const initialState: UserState = {
  users: []
};

export const userReducer = createReducer(
  initialState,
  on(addUser, (state, { user }) => ({
    ...state,
    users: [...state.users, user]
  }))
);
