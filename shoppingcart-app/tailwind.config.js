/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          50:  '#e8effe',
          100: '#c6d9fc',
          200: '#9dbcfa',
          300: '#6a98f6',
          400: '#3b78f0',
          500: '#1560e8',
          600: '#0E4FB9',
          700: '#0c44a0',
          800: '#0a3a8a',
          900: '#082f72',
        },
        nav: '#1C2434',
        dark: {
          100: '#1f2937',
          200: '#1a1f2e',
          300: '#111827',
          400: '#0f172a',
          500: '#0d1117',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
      container: {
        center: true,
        padding: '1rem',
        screens: {
          sm: '640px',
          md: '768px',
          lg: '1024px',
          xl: '1280px',
          '2xl': '1320px',
        },
      },

    },
  },
  plugins: [],
}
