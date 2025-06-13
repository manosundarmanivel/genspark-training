
export class RecipeModel {
  constructor(
    public id: number = 0,
    public name: string = '',
    public cuisine: string = '',
    public cookTimeMinutes: number = 0,
    public prepTimeMinutes: number = 0,
    public ingredients: string[] = [],
    public image: string = ''
  ) {}
}
