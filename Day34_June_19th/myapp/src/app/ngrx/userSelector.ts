
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UserState } from '../ngrx/userReducer';

export const selectUserState = createFeatureSelector<UserState>('userState');

export const selectAllUsers = createSelector(
  selectUserState,
  (state) => state.users
);
