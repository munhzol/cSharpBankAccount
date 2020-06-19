
#Check Duplicate Email
#Email is already in use, try logging in

> Add Extra Error state to A field

```
            if(ModelState.IsValid) {
                if(_context.Users.FirstOrDefault(u=>u.Email==newUser.Email) == null) {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    return RedirectToAction("Success");
                } else {
                    ModelState.AddModelError("Email", "Email is already in use, try logging in");
                    return View("Index");
                }
            }
            else
                return View("Index");
```