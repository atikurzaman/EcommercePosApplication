import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { User, Lock, Loader2, Save } from 'lucide-react';
import { useQuery, useMutation } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { authApi } from '@/api/authApi';
import apiClient from '@/api/client';

interface ProfileFormData {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  preferredLanguage: string;
  timeZone: string;
}

interface PasswordFormData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const emptyProfile: ProfileFormData = {
  firstName: '',
  lastName: '',
  phoneNumber: '',
  preferredLanguage: 'en',
  timeZone: 'UTC',
};

const emptyPassword: PasswordFormData = {
  currentPassword: '',
  newPassword: '',
  confirmPassword: '',
};

export default function UserProfile() {
  const [profileForm, setProfileForm] = useState<ProfileFormData>(emptyProfile);
  const [passwordForm, setPasswordForm] = useState<PasswordFormData>(emptyPassword);

  const { data: userData, isLoading } = useQuery({
    queryKey: ['current-user'],
    queryFn: () => authApi.getCurrentUser(),
  });

  const currentUser = (userData as any)?.data?.data || (userData as any)?.data;

  useEffect(() => {
    if (currentUser) {
      setProfileForm({
        firstName: currentUser.firstName || '',
        lastName: currentUser.lastName || '',
        phoneNumber: currentUser.phoneNumber || '',
        preferredLanguage: (currentUser as Record<string, unknown>).preferredLanguage as string || 'en',
        timeZone: (currentUser as Record<string, unknown>).timeZone as string || 'UTC',
      });
    }
  }, [currentUser]);

  const updateProfileMutation = useMutation({
    mutationFn: (data: ProfileFormData) => apiClient.put('/auth/profile', data),
    onSuccess: () => toast.success('Profile updated'),
    onError: () => toast.error('Failed to update profile'),
  });

  const changePasswordMutation = useMutation({
    mutationFn: (data: { currentPassword: string; newPassword: string }) =>
      apiClient.post('/auth/change-password', data),
    onSuccess: () => {
      toast.success('Password changed successfully');
      setPasswordForm(emptyPassword);
    },
    onError: () => toast.error('Failed to change password'),
  });

  const handleProfileSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateProfileMutation.mutate(profileForm);
  };

  const handlePasswordSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      toast.error('Passwords do not match');
      return;
    }
    if (passwordForm.newPassword.length < 6) {
      toast.error('Password must be at least 6 characters');
      return;
    }
    changePasswordMutation.mutate({
      currentPassword: passwordForm.currentPassword,
      newPassword: passwordForm.newPassword,
    });
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <Loader2 className="w-8 h-8 animate-spin" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="nx-page-header">
        <div>
          <h1 className="nx-page-title">My Profile</h1>
          <p className="nx-page-subtitle">Manage your account settings</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <div className="p-4 border-b flex items-center gap-2">
            <User className="w-4 h-4" />
            <h3 className="font-semibold">Profile Information</h3>
          </div>
          <form onSubmit={handleProfileSubmit} className="p-4 space-y-4">
            <div>
              <label className="text-sm font-medium">Email</label>
              <Input value={currentUser?.email || ''} disabled className="bg-muted" />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium">First Name</label>
                <Input
                  value={profileForm.firstName}
                  onChange={(e) => setProfileForm({ ...profileForm, firstName: e.target.value })}
                  placeholder="First name"
                />
              </div>
              <div>
                <label className="text-sm font-medium">Last Name</label>
                <Input
                  value={profileForm.lastName}
                  onChange={(e) => setProfileForm({ ...profileForm, lastName: e.target.value })}
                  placeholder="Last name"
                />
              </div>
            </div>
            <div>
              <label className="text-sm font-medium">Phone Number</label>
              <Input
                value={profileForm.phoneNumber}
                onChange={(e) => setProfileForm({ ...profileForm, phoneNumber: e.target.value })}
                placeholder="Phone number"
              />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="text-sm font-medium">Language</label>
                <select
                  className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm"
                  value={profileForm.preferredLanguage}
                  onChange={(e) => setProfileForm({ ...profileForm, preferredLanguage: e.target.value })}
                >
                  <option value="en">English</option>
                  <option value="bn">Bengali</option>
                  <option value="ar">Arabic</option>
                  <option value="es">Spanish</option>
                  <option value="fr">French</option>
                </select>
              </div>
              <div>
                <label className="text-sm font-medium">Time Zone</label>
                <select
                  className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm"
                  value={profileForm.timeZone}
                  onChange={(e) => setProfileForm({ ...profileForm, timeZone: e.target.value })}
                >
                  <option value="UTC">UTC</option>
                  <option value="Asia/Dhaka">Asia/Dhaka</option>
                  <option value="America/New_York">America/New_York</option>
                  <option value="America/Chicago">America/Chicago</option>
                  <option value="America/Los_Angeles">America/Los_Angeles</option>
                  <option value="Europe/London">Europe/London</option>
                  <option value="Asia/Tokyo">Asia/Tokyo</option>
                </select>
              </div>
            </div>
            <div className="flex justify-end pt-4 border-t">
              <Button type="submit" disabled={updateProfileMutation.isPending}>
                {updateProfileMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                <Save className="w-4 h-4 mr-2" />
                Save Profile
              </Button>
            </div>
          </form>
        </Card>

        <Card>
          <div className="p-4 border-b flex items-center gap-2">
            <Lock className="w-4 h-4" />
            <h3 className="font-semibold">Change Password</h3>
          </div>
          <form onSubmit={handlePasswordSubmit} className="p-4 space-y-4">
            <div>
              <label className="text-sm font-medium">Current Password *</label>
              <Input
                type="password"
                value={passwordForm.currentPassword}
                onChange={(e) => setPasswordForm({ ...passwordForm, currentPassword: e.target.value })}
                placeholder="Enter current password"
                required
              />
            </div>
            <div>
              <label className="text-sm font-medium">New Password *</label>
              <Input
                type="password"
                value={passwordForm.newPassword}
                onChange={(e) => setPasswordForm({ ...passwordForm, newPassword: e.target.value })}
                placeholder="Enter new password"
                required
              />
            </div>
            <div>
              <label className="text-sm font-medium">Confirm New Password *</label>
              <Input
                type="password"
                value={passwordForm.confirmPassword}
                onChange={(e) => setPasswordForm({ ...passwordForm, confirmPassword: e.target.value })}
                placeholder="Confirm new password"
                required
              />
            </div>
            <div className="flex justify-end pt-4 border-t">
              <Button type="submit" disabled={changePasswordMutation.isPending}>
                {changePasswordMutation.isPending && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                <Lock className="w-4 h-4 mr-2" />
                Change Password
              </Button>
            </div>
          </form>
        </Card>
      </div>
    </div>
  );
}
