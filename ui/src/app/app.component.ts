import { ChangeDetectionStrategy, Component, ElementRef, HostListener, computed, inject, signal, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { filter, map, startWith } from 'rxjs';

import { AuthSessionService } from '@app/core/services/auth-session.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, MatToolbarModule, MatButtonModule],
  templateUrl: './app.component.html',
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
    }
    .shell-toolbar {
      position: sticky;
      top: 0;
      z-index: 10;
      background: var(--app-toolbar);
      backdrop-filter: blur(16px);
      border-bottom: 1px solid var(--app-border);
    }
    .shell-toolbar__content {
      width: min(100%, var(--app-shell-width));
      margin: 0 auto;
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 0.85rem 1.5rem;
    }
    .shell-spacer { flex: 1; }
    .shell-content {
      width: min(100%, var(--app-shell-width));
      margin: 0 auto;
      min-height: calc(100vh - 73px);
      padding: 1.75rem 1.5rem 2.5rem;
    }
    .shell-content--auth {
      width: 100%;
      max-width: none;
      min-height: 100dvh;
      padding: 0;
    }
    .brand {
      color: var(--app-primary);
      text-decoration: none;
      font-weight: 700;
      letter-spacing: -0.02em;
    }
    .profile-trigger{
      display:inline-flex;
      align-items:center;
      justify-content:center;
      width:48px;
      min-width:48px;
      height:48px;
      padding:0;
      border:1px solid rgba(37, 99, 235, 0.16);
      border-radius:999px;
      background:rgba(255,255,255,0.92);
      box-shadow:var(--app-shadow-soft);
      color:var(--app-on-surface);
    }
    .profile-trigger:hover{
      background:#ffffff;
    }
    .profile-trigger__avatar{
      display:inline-flex;
      align-items:center;
      justify-content:center;
      width:38px;
      height:38px;
      border-radius:999px;
      background:linear-gradient(135deg, rgba(37, 99, 235, 0.16), rgba(20, 184, 166, 0.18));
      color:var(--app-primary);
      font-weight:800;
      font-size:0.95rem;
      letter-spacing:0.01em;
    }
    .profile-shell{
      position:relative;
      display:flex;
      align-items:center;
    }
    .profile-menu{
      position:absolute;
      top:calc(100% + 0.8rem);
      right:0;
      z-index:30;
      width:min(460px, calc(100vw - 1rem));
      max-width:min(460px, calc(100vw - 1rem));
      padding:0.35rem;
      overflow:hidden;
    }
    .profile-menu__card{
      display:grid;
      gap:0.9rem;
      padding:0.65rem;
      border-radius:1rem;
      border:1px solid var(--app-border);
      box-shadow:0 22px 48px rgba(15, 23, 42, 0.18);
      background:
        linear-gradient(180deg, rgba(248, 250, 252, 0.96), rgba(255, 255, 255, 0.98));
    }
    .profile-menu__hero{
      display:flex;
      align-items:center;
      gap:0.85rem;
      padding:0.35rem;
      border-radius:1rem;
      background:linear-gradient(135deg, rgba(37, 99, 235, 0.08), rgba(20, 184, 166, 0.08));
    }
    .profile-menu__hero-avatar{
      display:inline-flex;
      align-items:center;
      justify-content:center;
      width:48px;
      height:48px;
      border-radius:999px;
      background:#ffffff;
      color:var(--app-primary);
      font-size:1.05rem;
      font-weight:800;
      box-shadow:var(--app-shadow-soft);
    }
    .profile-menu__hero-copy{
      display:grid;
      gap:0.18rem;
      min-width:0;
      flex:1 1 auto;
    }
    .profile-menu__hero-title{
      font-size:0.96rem;
      font-weight:800;
      color:var(--app-on-surface);
      line-height:1.2;
      word-break:break-word;
    }
    .profile-menu__hero-subtitle{
      font-size:0.82rem;
      color:var(--app-on-muted);
      line-height:1.35;
      word-break:break-word;
      overflow-wrap:anywhere;
    }
    .profile-menu__details{
      display:grid;
      gap:0.75rem;
    }
    .profile-menu__item{
      display:grid;
      gap:0.18rem;
      padding:0.05rem 0.1rem;
    }
    .profile-menu__item-label{
      font-size:0.72rem;
      color:var(--app-on-muted);
      text-transform:uppercase;
      letter-spacing:0.08em;
      font-weight:700;
    }
    .profile-menu__item-value{
      color:var(--app-on-surface);
      font-size:0.92rem;
      line-height:1.4;
      max-width:100%;
      word-break:break-word;
      overflow-wrap:anywhere;
      white-space:normal;
    }
    .profile-menu__logout{
      width:100%;
      min-height:44px;
      border-radius:999px;
      font-weight:700;
      border:1px solid #fecaca !important;
      background:#fef2f2 !important;
      color:#b91c1c !important;
      box-shadow:none !important;
    }
    .profile-menu__logout:hover{
      background:#fee2e2 !important;
      border-color:#fca5a5 !important;
      color:#991b1b !important;
    }
    @media (max-width: 640px){
      .shell-toolbar__content{
        padding-inline:1rem;
      }
      .shell-content{
        padding-inline:1rem;
      }
      .profile-menu{
        right:-0.25rem;
        width:min(420px, calc(100vw - 0.75rem));
        max-width:min(420px, calc(100vw - 0.75rem));
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  private readonly router = inject(Router);
  protected readonly authSession = inject(AuthSessionService);
  private readonly profileShell = viewChild<ElementRef<HTMLElement>>('profileShell');
  protected readonly isProfileOpen = signal(false);
  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects),
      startWith(this.router.url)
    ),
    { initialValue: this.router.url }
  );
  protected readonly isPublicAuthRoute = computed(() => {
    const url = this.currentUrl();
    return url.startsWith('/login') || url.startsWith('/sign-up');
  });
  protected readonly currentUser = computed(() => this.authSession.session()?.user ?? null);
  protected readonly userInitial = computed(() => {
    const user = this.currentUser();
    return user?.username?.trim().charAt(0).toUpperCase() || 'U';
  });

  toggleProfileMenu(): void {
    this.isProfileOpen.update((value) => !value);
  }

  closeProfileMenu(): void {
    this.isProfileOpen.set(false);
  }

  logout(): void {
    this.closeProfileMenu();
    this.authSession.clearSession();
    void this.router.navigateByUrl('/login');
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.isProfileOpen()) {
      return;
    }

    const host = this.profileShell()?.nativeElement;
    const target = event.target as Node | null;

    if (host && target && !host.contains(target)) {
      this.closeProfileMenu();
    }
  }
}
