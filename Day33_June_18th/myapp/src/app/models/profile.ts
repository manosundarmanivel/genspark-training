export class ProfileModel {
  id!: number;
  username!: string;
  email!: string;
  firstName!: string;
  lastName!: string;
  gender!: 'male' | 'female' | string;
  image!: string;

  constructor(init?: Partial<ProfileModel>) {
    Object.assign(this, init);
  }
}
