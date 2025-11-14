# Compliance Documentation

This document outlines the compliance measures implemented in the Commentable API to meet WCAG 2.1 AA, ADA, HIPAA, and GDPR requirements.

## Table of Contents

1. [Accessibility Compliance (WCAG 2.1 AA / ADA)](#accessibility-compliance)
2. [HIPAA Compliance](#hipaa-compliance)
3. [GDPR Compliance](#gdpr-compliance)
4. [Internationalization (i18n)](#internationalization)
5. [Security Best Practices](#security-best-practices)
6. [Audit & Monitoring](#audit--monitoring)

---

## Accessibility Compliance (WCAG 2.1 AA / ADA)

### Implemented Features

#### 1. Semantic HTML & ARIA Labels
- **Button Component** (`apps/web/src/lib/components/ui/button.svelte`)
  - Supports `aria-label`, `aria-describedby`, `aria-expanded`, `aria-controls`, `aria-pressed`
  - Properly implements `aria-disabled` for disabled states
  - WCAG Success Criteria: 4.1.2 (Name, Role, Value)

#### 2. Form Accessibility
- **Input Component** (`apps/web/src/lib/components/ui/input.svelte`)
  - Supports `aria-label`, `aria-describedby`, `aria-invalid`, `aria-required`
  - Implements `autocomplete` attribute for autofill
  - Programmatic association with labels via `id` attribute
  - WCAG Success Criteria: 1.3.1 (Info and Relationships), 3.3.2 (Labels or Instructions)

#### 3. Keyboard Navigation
- **Reactions Component** (`apps/web/src/lib/components/reactions.svelte`)
  - Full keyboard support: Enter, Space, Escape, Arrow keys
  - Proper `tabindex` management for roving focus
  - `role="toolbar"` for reaction picker (replaces incorrect `role="menu"`)
  - `aria-pressed` for toggle buttons
  - WCAG Success Criteria: 2.1.1 (Keyboard), 2.1.2 (No Keyboard Trap)

#### 4. Screen Reader Support
- All emoji content marked with `role="img"` and `aria-hidden="true"`
- Text alternatives provided via `aria-label`
- Screen reader only content with `.sr-only` class
- WCAG Success Criteria: 1.1.1 (Non-text Content)

#### 5. Focus Indicators
- Visible focus rings on all interactive elements
- `focus-visible:ring-2` utility applied consistently
- WCAG Success Criteria: 2.4.7 (Focus Visible)

#### 6. Color Contrast
- All color combinations meet WCAG AA standard (4.5:1 ratio)
- Dark mode support with proper contrast ratios
- WCAG Success Criteria: 1.4.3 (Contrast Minimum)

#### 7. Responsive Design
- Mobile-first responsive layouts
- Text resizes without loss of functionality (up to 200%)
- WCAG Success Criteria: 1.4.4 (Resize Text), 1.4.10 (Reflow)

### Screen Reader Utilities

```css
/* apps/web/src/app.css */
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border-width: 0;
}

.sr-only-focusable:focus {
  /* Becomes visible when focused */
}

.skip-link {
  /* Skip to main content link for keyboard users */
}
```

---

## HIPAA Compliance

### Technical Safeguards Implemented

#### 1. Access Control (§164.312(a))

**Authentication & Authorization**
- JWT-based authentication with secure token validation
- Role-based access control (RBAC) for moderators
- Session management with token expiration
- Location: `apps/api/Program.cs:36-59`

#### 2. Audit Controls (§164.312(b))

**Audit Logging Middleware**
- Logs all API access: Who, What, When, Where
- Records user actions, IP addresses, timestamps
- Captures success/failure status
- Location: `apps/api/Infrastructure/Middleware/AuditLoggingMiddleware.cs`

```csharp
public class AuditLogEntry
{
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string IPAddress { get; set; }
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public int StatusCode { get; set; }
    public bool Success { get; set; }
}
```

**Production Requirements:**
- Audit logs must be stored in append-only database
- Implement log integrity protection (checksums/signatures)
- Retain logs for minimum 6 years
- Regular audit log reviews required

#### 3. Integrity Controls (§164.312(c))

**Security Headers Middleware**
- Location: `apps/api/Infrastructure/Middleware/SecurityHeadersMiddleware.cs`

Headers implemented:
```
Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
X-Frame-Options: DENY
X-Content-Type-Options: nosniff
Content-Security-Policy: [policy]
Cache-Control: no-store, no-cache, must-revalidate, private
```

#### 4. Transmission Security (§164.312(e))

- HTTPS enforcement via `UseHttpsRedirection()`
- TLS 1.2+ required for all connections
- HSTS header prevents downgrade attacks

#### 5. Encryption & Decryption (§164.312(a)(2)(iv))

**Data at Rest:**
- Database encryption required in production
- SQLite: Use SQLCipher or similar
- PostgreSQL: Enable transparent data encryption (TDE)

**Data in Transit:**
- All API communications over HTTPS/TLS
- JWT tokens encrypted and signed

#### 6. Password Management

- Bcrypt password hashing (cost factor: 12)
- Password complexity requirements recommended
- Location: `apps/api/Infrastructure/Services/Auth/JwtTokenService.cs`

### Physical Safeguards

**Required for Production:**
- Secure data center with physical access controls
- Workstation security policies
- Device and media controls
- Facility access controls and validation procedures

### Administrative Safeguards

**Required Policies:**
- Security Management Process
- Assigned Security Responsibility
- Workforce Security Training
- Information Access Management
- Security Incident Procedures
- Contingency Plan (data backup and disaster recovery)
- Business Associate Agreements (if applicable)

---

## GDPR Compliance

### Data Protection Principles

#### 1. Lawfulness, Fairness & Transparency (Art. 5(1)(a))

**Required Implementation (TODO):**
- Cookie consent banner
- Privacy policy clearly displayed
- Terms of service acceptance
- Data processing notices

#### 2. Purpose Limitation (Art. 5(1)(b))

- Data collected only for specified purposes
- User data limited to: authentication, content creation, moderation
- No secondary use without consent

#### 3. Data Minimization (Art. 5(1)(c))

Current data collection:
- User: email, username, password (hashed), role
- Content: videos, posts, comments, reactions
- Audit: IP addresses, timestamps, user actions

**Recommendations:**
- Avoid collecting unnecessary PII
- Consider pseudonymization for analytics

#### 4. Accuracy (Art. 5(1)(d))

- Users can update their profile data
- Soft delete maintains data integrity
- Correction mechanisms in place

#### 5. Storage Limitation (Art. 5(1)(e))

**Required Policies (TODO):**
- Define data retention periods
- Automated deletion of old data
- Exception handling for legal holds

#### 6. Integrity & Confidentiality (Art. 5(1)(f))

- Implemented via HIPAA security measures above
- Encryption, access controls, audit logging

### Data Subject Rights (Art. 12-23)

**Required API Endpoints (TODO):**

1. **Right to Access (Art. 15)**
   - `GET /api/users/me/data-export`
   - Returns all user data in machine-readable format (JSON)

2. **Right to Rectification (Art. 16)**
   - `PATCH /api/users/me`
   - Allow users to correct their data

3. **Right to Erasure (Art. 17)**
   - `DELETE /api/users/me`
   - Implement "right to be forgotten"
   - Anonymize content or delete account

4. **Right to Data Portability (Art. 20)**
   - Export user data in JSON/CSV format
   - Include all posts, comments, reactions

5. **Right to Object (Art. 21)**
   - Opt-out of data processing
   - Marketing communications opt-out

### Consent Management (Art. 7)

**Required Implementation (TODO):**
- Granular consent options
- Consent withdrawal mechanism
- Consent audit trail
- Clear "Accept" and "Decline" options

### Data Breach Notification (Art. 33-34)

**Required Procedures:**
- Breach detection within 72 hours
- Notification to supervisory authority
- Notification to affected individuals if high risk
- Breach documentation and reporting

---

## Internationalization (i18n)

### TODO: Implementation Required

**Recommended Library:** `svelte-i18n`

#### Setup Instructions

1. Install dependencies:
```bash
npm install svelte-i18n
```

2. Create translation files:
```
apps/web/src/lib/i18n/
  ├── en.json
  ├── es.json
  ├── fr.json
  └── de.json
```

3. Initialize i18n:
```typescript
// apps/web/src/lib/i18n/index.ts
import { init, register, locale } from 'svelte-i18n';

register('en', () => import('./en.json'));
register('es', () => import('./es.json'));
register('fr', () => import('./fr.json'));
register('de', () => import('./de.json'));

init({
  fallbackLocale: 'en',
  initialLocale: getBrowserLocale()
});
```

4. Use in components:
```svelte
<script>
  import { _ } from 'svelte-i18n';
</script>

<h1>{$_('dashboard.welcome')}</h1>
<Button aria-label={$_('buttons.upload')}>{$_('buttons.upload')}</Button>
```

#### Translation Keys Needed

- Navigation labels
- Button labels (for `aria-label`)
- Error messages
- Success messages
- Form labels and placeholders
- Help text and descriptions
- Reaction labels (Like, Love, Laugh, etc.)

#### WCAG Compliance

- `lang` attribute on `<html>` element
- `hreflang` for language switcher
- Text direction support (RTL for Arabic, Hebrew)
- WCAG Success Criteria: 3.1.1 (Language of Page), 3.1.2 (Language of Parts)

---

## Security Best Practices

### Content Security Policy (CSP)

Implemented in `SecurityHeadersMiddleware.cs`:

```
default-src 'self';
script-src 'self' 'unsafe-inline' 'unsafe-eval';
style-src 'self' 'unsafe-inline';
img-src 'self' data: https:;
font-src 'self' data:;
connect-src 'self' http://localhost:*;
frame-ancestors 'none';
upgrade-insecure-requests;
```

**Production Recommendations:**
- Remove `'unsafe-inline'` and `'unsafe-eval'`
- Use nonces or hashes for inline scripts
- Whitelist specific external domains

### Cross-Site Scripting (XSS) Prevention

- Input sanitization on backend
- Output encoding in frontend (Svelte does this automatically)
- CSP headers prevent injection attacks

### Cross-Site Request Forgery (CSRF)

- SameSite cookie attribute (if using cookies)
- JWT tokens in Authorization header (not vulnerable to CSRF)

### SQL Injection Prevention

- Entity Framework Core parameterized queries
- No raw SQL queries without parameterization

### Rate Limiting (TODO)

**Recommended Implementation:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
    });
});
```

---

## Audit & Monitoring

### Logging Strategy

#### Development
- Console logging enabled
- SQLite database audit logs
- Swagger UI for API testing

#### Production Requirements

1. **Centralized Logging**
   - Use Seq, ELK Stack, or Azure Application Insights
   - Structured logging with Serilog

2. **Audit Log Database**
   - Separate append-only database for audit logs
   - Daily backups with integrity verification
   - Minimum 6-year retention (HIPAA)

3. **Monitoring Alerts**
   - Failed login attempts (threshold: 5 in 5 minutes)
   - Unauthorized access attempts
   - Data export requests
   - Account deletion requests
   - System errors and exceptions

4. **Performance Monitoring**
   - API response times
   - Database query performance
   - Error rates
   - Uptime monitoring

### Security Incident Response

**Required Procedures:**
1. Incident detection and reporting
2. Containment and eradication
3. Recovery and restoration
4. Post-incident analysis
5. Documentation and lessons learned

---

## Compliance Checklist

### WCAG 2.1 AA / ADA
- [x] Semantic HTML with ARIA labels
- [x] Keyboard navigation support
- [x] Screen reader compatibility
- [x] Color contrast ratios (4.5:1)
- [x] Focus indicators
- [x] Responsive design
- [ ] i18n language support
- [ ] Skip to main content link implementation
- [ ] Automated accessibility testing (axe-core, Pa11y)

### HIPAA
- [x] Access controls (authentication/authorization)
- [x] Audit logging middleware
- [x] Security headers
- [x] HTTPS/TLS encryption
- [x] Password hashing (Bcrypt)
- [ ] Encryption at rest (production)
- [ ] Business Associate Agreements
- [ ] Workforce training documentation
- [ ] Contingency/backup plan
- [ ] Security incident procedures

### GDPR
- [ ] Cookie consent banner
- [ ] Privacy policy
- [ ] Terms of service
- [ ] Data export API (Right to Access)
- [ ] Account deletion (Right to Erasure)
- [ ] Data portability API
- [ ] Consent management system
- [ ] Data retention policies
- [ ] Breach notification procedures
- [ ] Data Processing Agreement (DPA) templates

### Security
- [x] CORS configuration
- [x] JWT authentication
- [x] Input validation
- [x] CSP headers
- [ ] Rate limiting
- [ ] Penetration testing
- [ ] Vulnerability scanning
- [ ] Security audit (annual)

---

## Maintenance & Review

### Regular Reviews Required

- **Quarterly:** Security patch updates
- **Semi-Annual:** Audit log review
- **Annual:** Full security audit and penetration testing
- **Annual:** HIPAA risk assessment
- **Continuous:** Monitor for CVEs in dependencies

### Dependency Updates

```bash
# Frontend
cd apps/web
npm audit
npm update

# Backend
cd apps/api
dotnet list package --vulnerable
dotnet outdated
```

---

## Contact & Responsibility

**Security Officer:** [Name/Email]
**Privacy Officer:** [Name/Email]
**Compliance Team:** [Email]

**Report Security Issues:** security@commentable.com
**Report Privacy Concerns:** privacy@commentable.com

---

## References

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [ADA Title III](https://www.ada.gov/regs2010/titleIII_2010/titleIII_2010_regulations.htm)
- [HIPAA Security Rule](https://www.hhs.gov/hipaa/for-professionals/security/index.html)
- [GDPR Full Text](https://gdpr-info.eu/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)

---

**Last Updated:** 2025-11-14
**Version:** 1.0.0
