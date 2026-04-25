import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar';
import { ToastComponent } from './shared/components/toast/toast';
import { DialogComponent } from './shared/components/dialog/dialog';
import { MailWidgetComponent } from './shared/components/mail-widget/mail-widget';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, ToastComponent, DialogComponent, MailWidgetComponent],
  template: `
    <app-navbar></app-navbar>
    <app-toast></app-toast>
    <app-dialog></app-dialog>
    <app-mail-widget></app-mail-widget>
    <router-outlet></router-outlet>
  `,
  styles: []
})
export class App {}
