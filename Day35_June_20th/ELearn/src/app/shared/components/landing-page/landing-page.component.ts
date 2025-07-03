import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './landing-page.component.html',
})
export class LandingPageComponent {



    featuredCourses = [
  {
    title: 'Mastering Web Development',
    instructor: 'John Doe',
    image: 'https://source.unsplash.com/400x200/?coding',
    price: '$49.99',
  },
  {
    title: 'Business Strategy Essentials',
    instructor: 'Jane Smith',
    image: 'https://source.unsplash.com/400x200/?business',
    price: '$39.99',
  },
  {
    title: 'UI/UX Design for Beginners',
    instructor: 'Sarah Lee',
    image: 'https://source.unsplash.com/400x200/?design',
    price: '$29.99',
  },
];

testimonials = [
  {
    name: 'Priya R.',
    role: 'Marketing Student',
    feedback: 'This platform completely changed the way I learn. The content quality is top-notch!',
  },
  {
    name: 'Raj K.',
    role: 'Instructor',
    feedback: 'As an instructor, it’s the best experience I’ve had. The interface is easy and powerful.',
  },
  {
    name: 'Anjali M.',
    role: 'Frontend Developer',
    feedback: 'Affordable courses with amazing mentors. I got a new job thanks to these lessons!',
  },
];

}
